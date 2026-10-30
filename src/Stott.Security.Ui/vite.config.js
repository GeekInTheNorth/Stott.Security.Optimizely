import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  base: './', // relative asset urls so fonts resolve under /stott.security.optimizely/static/
  build: {
    chunkSizeWarningLimit: 600, // value in kB, default is 500
  }
})
