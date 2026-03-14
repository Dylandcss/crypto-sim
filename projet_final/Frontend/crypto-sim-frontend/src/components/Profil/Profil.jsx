import { useAuth } from '../../context/AuthContext';
import { formatBalance } from '../../utils/formatters';
import styles from './Profil.module.css';

export default function Profil() {
  const { user } = useAuth();

  return (
    <div className={styles['profil-page']}>
      <h1>Profil</h1>
      <div className={styles['profil-info']}>
        <span className={styles.label}>Nom utilisateur</span>
        <div className={styles['info-row']}>
          <span className={styles.value}>{user?.username}</span>
        </div>

        <span className={styles.label}>Rôle</span>
        <div className={styles['info-row']}>
          <span className={styles.value}>{user?.role}</span>
        </div>

        <span className={styles.label}>Mon solde</span>
        <div className={styles['info-row']}>
          <span className={styles.value}>{formatBalance(user?.balance)}</span>
        </div>
      </div>
    </div>
  );
}
