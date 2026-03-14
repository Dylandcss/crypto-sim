import { useAuth } from '../../context/AuthContext';
import { getProfile } from '../../services/authService';
import { formatBalance } from '../../utils/formatters';
import useFetch from '../../hooks/useFetch';
import Loader from '../common/Loader/Loader';
import DisplayMessage from '../common/DisplayMessage/DisplayMessage';
import { User, Wallet, Calendar, Shield, Mail } from '@nsmr/pixelart-react';
import styles from './Profil.module.css';

const ROLE_MAP = {
  'User': 'Utilisateur',
  'Admin': 'Administrateur',
  'Standard': 'Standard'
};

export default function Profil() {
  const { user } = useAuth();
  const { data: profile, loading, error } = useFetch(getProfile);

  const displayRole = ROLE_MAP[user?.role] || user?.role;

  if (loading) return <Loader message="Chargement du profil..." />;
  if (error) return <DisplayMessage type="error" message={error} />;

  return (
    <div className={styles.profileCard}>
      <div className={styles.headerSection}>
        <div className={styles.avatar}>
          <User size={56} />
        </div>

        <div className={styles.userSection}>
          <h2 className={styles.username}>{user?.username}</h2>

          <div className={styles.metaGrid}>
            <div className={styles.metaItem}>
              <Mail size={14} />
              <span>{profile?.email}</span>
            </div>
            <div className={styles.metaItem}>
              <Shield size={14} />
              <span>Rôle : <strong>{displayRole}</strong></span>
            </div>
            <div className={styles.metaItem}>
              <Calendar size={14} />
              <span>Membre depuis le {new Date(profile?.createdAt).toLocaleDateString('fr-FR', { day: 'numeric', month: 'long', year: 'numeric' })}</span>
            </div>
          </div>
        </div>
      </div>

      <div className={styles.divider}></div>

      <div className={styles.balanceArea}>
        <div className={styles.balanceHeader}>
          <Wallet size={18} />
          <span className={styles.balanceLabel}>Mon solde disponible</span>
        </div>
        <h3 className={styles.balanceValue}>{formatBalance(user?.balance)}</h3>
      </div>
    </div>
  );
}
