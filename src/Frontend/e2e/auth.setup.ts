import { test as setup, expect } from '@playwright/test';
import path from 'path';

const authFile = path.join(__dirname, '../.playwright/.auth/user.json');

setup('authenticate', async ({ page }) => {
  const timestamp = Date.now();
  const email = `e2e-${timestamp}@example.com`;
  const password = 'E2ETest123!';

  await page.goto('/register');
  await page.getByLabel('Name').fill('E2E Test User');
  await page.getByLabel('Email').fill(email);
  await page.getByLabel('Password').fill(password);
  await page.getByRole('button', { name: /register/i }).click();

  await expect(page).toHaveURL(/\/products/, { timeout: 10000 });

  await page.context().storageState({ path: authFile });
});
