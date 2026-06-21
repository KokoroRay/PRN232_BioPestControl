/** Microservice base URLs — override via .env */
const isProd = import.meta.env.PROD;

export const API = {
  identity: import.meta.env.VITE_IDENTITY_API || (isProd ? '' : 'http://localhost:5240'),
  catalog: import.meta.env.VITE_CATALOG_API || (isProd ? '' : 'http://localhost:5123'),
  ordering: import.meta.env.VITE_ORDERING_API || (isProd ? '' : 'http://localhost:5112'),
  trading: import.meta.env.VITE_TRADING_API || (isProd ? '' : 'http://localhost:5071'),
  article: import.meta.env.VITE_ARTICLE_API || (isProd ? '' : 'http://localhost:5286'),
  agriExpert: import.meta.env.VITE_AGRI_API || (isProd ? '' : 'http://localhost:5050'),
  inventory: import.meta.env.VITE_INVENTORY_API || (isProd ? '' : 'http://localhost:5256'),
} as const;
