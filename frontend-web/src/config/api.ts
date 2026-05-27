/** Microservice base URLs — override via .env */
export const API = {
  identity: import.meta.env.VITE_IDENTITY_API ?? 'http://localhost:5240',
  catalog: import.meta.env.VITE_CATALOG_API ?? 'http://localhost:5123',
  ordering: import.meta.env.VITE_ORDERING_API ?? 'http://localhost:5112',
  trading: import.meta.env.VITE_TRADING_API ?? 'http://localhost:5071',
  article: import.meta.env.VITE_ARTICLE_API ?? 'http://localhost:5286',
  agriExpert: import.meta.env.VITE_AGRI_API ?? 'http://localhost:5050',
  inventory: import.meta.env.VITE_INVENTORY_API ?? 'http://localhost:5256',
} as const;
