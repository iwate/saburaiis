import child_process from 'child_process'
import fs from 'fs'
import path from 'path'
import { env, exit } from 'process'
import 'dotenv/config'

import { defineConfig, UserConfigExport } from 'vite'
import react from '@vitejs/plugin-react'

const baseFolder
  = env.APPDATA !== undefined && env.APPDATA !== ''
    ? `${env.APPDATA}/ASP.NET/https`
    : `${env.HOME}/.aspnet/https`

const certificateName = 'Commerble.Manage.Web.client'
const certFilePath = path.join(baseFolder, `${certificateName}.pem`)
const keyFilePath = path.join(baseFolder, `${certificateName}.key`)

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
  if (0 !== child_process.spawnSync('dotnet', [
    'dev-certs',
    'https',
    '--export-path',
    certFilePath,
    '--format',
    'Pem',
    '--no-password',
  ], { stdio: 'inherit' }).status) {
    throw new Error('Could not create certificate.')
  }
}

const target =
  env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
    : env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0]
      : null;

if (!target) {
  console.error('ASPNETCORE_HTTPS_PORT or ASPNETCORE_URLS environment variables should be set.');
  exit(1);
}

console.log(`Proxy target: ${target}`)

// https://vite.dev/config/
export default defineConfig({
  build: {
    target: ['es2023'],
  },
  plugins: [react()],
  server: {
    proxy: {
      '^/api/': {
        target,
        secure: false,
      },
    },
    port: 5173,
    https: {
      key: fs.readFileSync(keyFilePath),
      cert: fs.readFileSync(certFilePath),
    },
  },
  test: {
    include: [
      'src/**/*.{test,spec}.{js,mjs,cjs,ts,mts,cts,jsx,tsx}',
      'src/**/__tests__/*.{js,mjs,cjs,ts,mts,cts,jsx,tsx}',
    ],
  },
} as UserConfigExport)
