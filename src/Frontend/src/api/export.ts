import { api } from './client';

export interface SectionValidation {
  section: string;
  isValid: boolean;
  warnings: string[];
}

export const exportApi = {
  validate: (productId: string) =>
    api.get<{ validations: SectionValidation[] }>(`/products/${productId}/validate`),

  markdown: (productId: string) =>
    api.get(`/products/${productId}/export/markdown`, { responseType: 'blob' }),

  pdf: (productId: string) =>
    api.get(`/products/${productId}/export/pdf`, { responseType: 'blob' }),
};

export const downloadBlob = (blob: Blob, filename: string) => {
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
  URL.revokeObjectURL(url);
};
