import React from 'react'
import ReactDOM from 'react-dom/client'
import './index.css'
import { AxiomProvider } from '@optiaxiom/react'
import App from './App.jsx'

ReactDOM.createRoot(document.getElementById('security-app')).render(
  <React.StrictMode>
    <AxiomProvider>
      <App />
    </AxiomProvider>
  </React.StrictMode>,
)
