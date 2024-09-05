import { test, expect } from "@playwright/test";
import AxeBuilder from "@axe-core/playwright";

test("accessibility products", async ({ page }) => {
  await page.goto("https://raienryuu.github.io/EcommercePlatform/#/products");
  const accessibilityScanResults = await new AxeBuilder({ page }).analyze();

  expect(accessibilityScanResults.violations).toEqual([]);
});
test("accessibility login", async ({ page }) => {
  await page.goto("https://raienryuu.github.io/EcommercePlatform/#/login");
  const accessibilityScanResults = await new AxeBuilder({ page }).analyze();

  expect(accessibilityScanResults.violations).toEqual([]);
});
test("accessibility register", async ({ page }) => {
  await page.goto("https://raienryuu.github.io/EcommercePlatform/#/register");
  const accessibilityScanResults = await new AxeBuilder({ page }).analyze();

  expect(accessibilityScanResults.violations).toEqual([]);
});
test("accessibility cart", async ({ page }) => {
  await page.goto("https://raienryuu.github.io/EcommercePlatform/#/cart");
  const accessibilityScanResults = await new AxeBuilder({ page }).analyze();

  expect(accessibilityScanResults.violations).toEqual([]);
});
test("accessibility checkout", async ({ page }) => {
  await page.goto("https://raienryuu.github.io/EcommercePlatform/#/checkout");
  const accessibilityScanResults = await new AxeBuilder({ page }).analyze();

  expect(accessibilityScanResults.violations).toEqual([]);
});
