import { useCRT } from '../../context/CRTContext'
import styles from './CRTOverlay.module.css'
import './CRTGlobal.css'

export default function CRTOverlay() {
  const { enabled } = useCRT()

  if (!enabled) return null

  return (
    <div className={styles.crt} aria-hidden="true">
      <div className={styles.scanlines} />
      <div className={styles.phosphor} />
      <div className={styles.glitch} />
      <div className={styles.sweep} />
      <div className={styles.flicker} />
    </div>
  )
}
