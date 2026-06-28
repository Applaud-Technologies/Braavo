import { test, expect, request } from '@playwright/test';

const API_URL = 'http://localhost:5153/api';

test.describe('Phase 5: Export & Preview', () => {
  let authToken: string;
  let authUser: { id: string; email: string; name: string };
  let productId: string;

  test.beforeAll(async () => {
    const apiContext = await request.newContext();
    const timestamp = Date.now();
    const email = `e2e-${timestamp}@example.com`;
    const password = 'E2ETest123!';

    // Register via API
    let registerRes = await apiContext.post(`${API_URL}/auth/register`, {
      data: { email, password, name: 'E2E Test User' },
    });

    // If registration fails (email taken), try login
    if (!registerRes.ok()) {
      registerRes = await apiContext.post(`${API_URL}/auth/login`, {
        data: { email, password },
      });
    }

    const registerData = await registerRes.json();
    authToken = registerData.token;
    authUser = registerData.user;

    // Create product via API
    const productRes = await apiContext.post(`${API_URL}/products`, {
      headers: { Authorization: `Bearer ${authToken}` },
      data: {
        name: `Test Product ${timestamp}`,
        description: 'E2E test product for Phase 5',
      },
    });
    const productData = await productRes.json();
    productId = productData.productId;

    await apiContext.dispose();
  });

  test.beforeEach(async ({ page }) => {
    await page.goto('/login');
    await page.evaluate(
      ({ token, user }) => {
        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(user));
      },
      { token: authToken, user: authUser }
    );
    await page.reload();
  });

  test('product detail page shows preview link', async ({ page }) => {
    await page.goto(`/products/${productId}`);
    await page.waitForLoadState('networkidle');

    const previewLink = page.locator('a[href*="preview"]');
    await expect(previewLink).toBeVisible({ timeout: 5000 });
  });

  test('product detail page shows validation warnings', async ({ page }) => {
    await page.goto(`/products/${productId}`);
    await page.waitForLoadState('networkidle');

    // Empty product should show warnings
    const warningBanner = page.locator('[class*="amber"]');
    await expect(warningBanner.first()).toBeVisible({ timeout: 10000 });
  });

  test('product detail page shows completion percentage', async ({ page }) => {
    await page.goto(`/products/${productId}`);
    await page.waitForLoadState('networkidle');

    // Look for progress bar
    const progressBar = page.locator('[class*="bg-primary"]');
    await expect(progressBar.first()).toBeVisible({ timeout: 10000 });
  });

  test('API: validation endpoint returns section validations', async () => {
    const apiContext = await request.newContext();
    const res = await apiContext.get(`${API_URL}/products/${productId}/validate`, {
      headers: { Authorization: `Bearer ${authToken}` },
    });

    expect(res.ok()).toBeTruthy();
    const data = await res.json();
    expect(data.validations).toBeDefined();
    expect(data.validations.length).toBe(5);
    await apiContext.dispose();
  });

  test('API: markdown export returns file', async () => {
    const apiContext = await request.newContext();
    const res = await apiContext.get(`${API_URL}/products/${productId}/export/markdown`, {
      headers: { Authorization: `Bearer ${authToken}` },
    });

    expect(res.ok()).toBeTruthy();
    expect(res.headers()['content-type']).toBe('text/markdown');
    expect(res.headers()['content-disposition']).toContain('.md');
    await apiContext.dispose();
  });

  test('API: PDF export returns file', async () => {
    const apiContext = await request.newContext();
    const res = await apiContext.get(`${API_URL}/products/${productId}/export/pdf`, {
      headers: { Authorization: `Bearer ${authToken}` },
    });

    expect(res.ok()).toBeTruthy();
    expect(res.headers()['content-type']).toBe('application/pdf');
    expect(res.headers()['content-disposition']).toContain('.pdf');
    await apiContext.dispose();
  });
});
