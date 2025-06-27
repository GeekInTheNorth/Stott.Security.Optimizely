import { } from 'react'
import './App.css'
import { CspProvider } from './csp/CspContext'
import EditCorsSettings from './cors/EditCorsSettings'
import CspContainer from './csp/CspContainer'

function App() {

  return (
    <CspProvider>
      <CspContainer />
      <EditCorsSettings />
    </CspProvider>
  )
}

export default App
