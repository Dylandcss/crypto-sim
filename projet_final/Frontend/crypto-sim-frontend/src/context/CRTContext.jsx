import { createContext, useContext, useState, useEffect } from 'react'

const STORAGE_KEY = 'crt_enabled'
const CRTContext = createContext(null)

export function CRTProvider({ children }) {
  const [enabled, setEnabled] = useState(
    () => localStorage.getItem(STORAGE_KEY) !== 'false'
  )

  useEffect(() => {
    localStorage.setItem(STORAGE_KEY, String(enabled))
    document.body.classList.toggle('crt-active', enabled)
    const root = document.getElementById('app-root')
    if (root) root.classList.toggle('crt-active', enabled)
    return () => {
      document.body.classList.remove('crt-active')
      document.getElementById('app-root')?.classList.remove('crt-active')
    }
  }, [enabled])

  return (
    <CRTContext.Provider value={{ enabled, toggle: () => setEnabled(v => !v) }}>
      {children}
    </CRTContext.Provider>
  )
}

export function useCRT() {
  return useContext(CRTContext)
}
