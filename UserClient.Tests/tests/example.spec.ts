import { test, expect } from "@playwright/test";

test("regression products", async ({ page }) => {
  await page.goto("http://localhost:4200");
  await expect(page).toHaveScreenshot();
});
test("regression login", async ({ page }) => {
  await page.goto("http://localhost:4200");
  await expect(page).toHaveScreenshot();
});
test("regression register", async ({ page }) => {
  await page.goto("http://localhost:4200");
  await expect(page).toHaveScreenshot();
});
test("regression cart", async ({ page }) => {
  await page.goto("http://localhost:4200");
  await expect(page).toHaveScreenshot();
});
test("regression checkout", async ({ page }) => {
  await page.goto("http://localhost:4200");
  await expect(page).toHaveScreenshot();
});


