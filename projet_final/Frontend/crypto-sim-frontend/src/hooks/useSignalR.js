import { useState, useEffect, useRef } from 'react';
import { createSignalRConnection } from '../services/marketService';

/**
 * Gère le cycle de vie d'une connexion SignalR.
 * @param {function} onPriceUpdate - Callback appelé à chaque mise à jour de prix.
 * @returns {{ error: string|null }}
 */
export default function useSignalR(onPriceUpdate) {
  const [error, setError] = useState(null);

  const callbackRef = useRef(onPriceUpdate);

  useEffect(() => {
    callbackRef.current = onPriceUpdate;
  });

  useEffect(() => {
    let isCancelled = false;

    const connection = createSignalRConnection(
      (updates) => callbackRef.current?.(updates),
      () => {
        if (!isCancelled) {
          setError('Connexion définitivement fermée.');
        }
      }
    );

    connection.onreconnecting(() => {
      if (!isCancelled) {
        setError('Reconnexion en cours...');
      }
    });

    connection.onreconnected(() => {
      if (!isCancelled) {
        setError(null);
      }
    });

    connection
      .start()
      .then(() => {
        if (!isCancelled) setError(null);
      })
      .catch(() => {
        if (!isCancelled) {
          setError('Impossible de se connecter...');
        }
      });

    return () => {
      isCancelled = true;
      connection.stop().catch(() => {});
    };
  }, []);

  return { error };
}
