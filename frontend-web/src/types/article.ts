export interface Article {
  id: string;
  title: string;
  content: string;
  summary?: string;
  thumbnailUrl?: string;
  status: string;
  tags?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateArticleRequest {
  title: string;
  content: string;
  summary?: string;
  thumbnailUrl?: string;
  status: string;
  tags?: string;
}
