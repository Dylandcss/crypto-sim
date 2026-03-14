import { useState, useEffect, useRef } from 'react';
import { createSignalRConnection } from '../services/marketService';

/**
 * Gère le cycle de vie d'une connexion SignalR.
 * @param {function} onPriceUpdate - Callback appelé à chaque mise à jour de prix.
 * @returns {{ isLive: boolean, error: string|null }}
 */
export default function useSignalR(onPriceUpdate) {
  const [isLive, setIsLive] = useState(false);
  const [error, setError] = useState(null);
  const callbackRef = useRef(onPriceUpdate);

  // Toujours garder la référence au dernier callback sans recréer la connexion
  useEffect(() => {
    callbackRef.current = onPriceUpdate;
  });

  useEffect(() => {
    const connection = createSignalRConnection(
      (updates) => callbackRef.current?.(updates),
      (err) => {
        if (err) {
          setIsLive(false);
          setError('Connexion perdue, reconnexion en cours...');
        }
      }
    );

    connection
      .start()
      .then(() => {
        setIsLive(true);
        setError(null);
      })
      .catch(() => {
        setError('Impossible de se connecter au flux de prix en temps réel.');
      });

    return () => {
      connection.stop();
      setIsLive(false);
    };
  }, []);

  return { isLive, error };
}
