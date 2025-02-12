import { test, expect } from "@playwright/test";

test("regression products", async ({ page }) => {
  await page.goto("https://raienryuu.github.io/EcommercePlatform/#/products");
  await expect(page).toHaveScreenshot();
});
test("regression login", async ({ page }) => {
  await page.goto("https://raienryuu.github.io/EcommercePlatform/#/login");
  await expect(page).toHaveScreenshot();
});
test("regression register", async ({ page }) => {
  await page.goto("https://raienryuu.github.io/EcommercePlatform/#/register");
  await expect(page).toHaveScreenshot();
});
test("regression cart", async ({ page }) => {
  await page.goto("https://raienryuu.github.io/EcommercePlatform/#/cart");
  await expect(page).toHaveScreenshot();
});
test("regression checkout", async ({ page }) => {
  await page.goto("https://raienryuu.github.io/EcommercePlatform/#/checkout");
  await expect(page).toHaveScreenshot();
});
test("regression details", async ({ page }) => {
  await page.goto("https://raienryuu.github.io/EcommercePlatform/#/details/1");
  await expect(page).toHaveScreenshot();
});

