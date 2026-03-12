import React from 'react';
import styles from './Loader.module.css';

const Loader = ({ message = 'Chargement en cours...', fullScreen = false }) => {
  return (
    <div className={`${styles.loaderContainer} ${fullScreen ? styles.fullScreen : ''}`}>
      <div className={styles.spinner}></div>
      {message && <p className={styles.message}>{message}</p>}
    </div>
  );
};

export default Loader;
