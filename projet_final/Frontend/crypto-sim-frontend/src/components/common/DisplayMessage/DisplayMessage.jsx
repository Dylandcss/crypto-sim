import styles from './DisplayMessage.module.css';

const DisplayMessage = ({ type = 'info', title, message, onClose }) => {
  // Types supportés: 'info', 'success', 'warning', 'error'
  const typeClass = styles[type] || styles.info;

  return (
    <div className={`${styles.messageBox} ${typeClass}`}>
      <div className={styles.content}>
        {title && <h4 className={styles.title}>{title}</h4>}
        {message && <p className={styles.text}>{message}</p>}
      </div>
      {onClose && (
        <button className={styles.closeBtn} onClick={onClose} aria-label="Fermer">
          &times;
        </button>
      )}
    </div>
  );
};

export default DisplayMessage;
