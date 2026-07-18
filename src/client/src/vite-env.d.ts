/// <reference types="vite/client" />

interface ViteTypeOptions {
  // By adding this line ImportMetaEnv is strict to disallow unknown keys
  strictImportMetaEnv: unknown
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}

interface ImportMetaEnv {
  readonly VITE_APP_TITLE: string
  readonly VITE_APP_VERSION: string
  
  readonly VITE_PROJECT_REPOSITORY_URL: string
  readonly VITE_DEVELOPER_GITHUB_URL: string
  readonly VITE_PROJECT_LICENSE_URL: string
  readonly VITE_PROJECT_ISSUES_URL: string

  /** When "true", starts the MSW worker at bootstrap so the GraphQL API is mocked. */
  readonly VITE_ENABLE_API_MOCKING: string
}
