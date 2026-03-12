import styles from './Profil.module.css'

export default function Profil() {
  return (
    <>
      <div className={styles['profil-page']}>
        <h1>Profil Page</h1>
        <div className={styles['profil-info']}>
          <span className={styles.label}>Nom utilisateur</span>
          <div className={styles['info-row']}>
            <span className={styles.value}>helene</span>
          </div>

          <span className={styles.label}>Email</span>
          <div className={styles['info-row']}>
            <span className={styles.value}>helene@mail.com</span>
          </div>

          <span className={styles.label}>Role</span>
          <div className={styles['info-row']}>
            <span className={styles.value}>User</span>
          </div>

          <span className={styles.label}>Mon solde</span>
          <div className={styles['info-row']}>
            <span className={styles.value}>100 000 000</span>
          </div>
        </div>
      </div>
    </>
  )
}
