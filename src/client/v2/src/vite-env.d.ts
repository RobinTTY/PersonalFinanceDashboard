/// <reference types="vite/client" />

interface ViteTypeOptions {
  // By adding this line ImportMetaEnv is strict to disallow unknown keys
  strictImportMetaEnv: unknown
}

interface ImportMetaEnv {
  readonly VITE_APP_TITLE: string
  readonly VITE_APP_VERSION: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}