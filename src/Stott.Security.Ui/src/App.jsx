import { } from 'react'
import './App.css'
import { CspProvider } from './csp/CspContext'
import CspContainer from './csp/CspContainer'

function App() {

  return (
    <CspProvider>
      <CspContainer />
    </CspProvider>
  )
}

export default App
