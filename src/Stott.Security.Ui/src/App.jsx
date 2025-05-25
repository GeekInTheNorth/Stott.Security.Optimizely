import { } from 'react'
import './App.css'
import CspList from './csp/CspList'
import { CspProvider } from './csp/CspContext'

function App() {

  return (
    <CspProvider>
      <CspList />
    </CspProvider>
  )
}

export default App
