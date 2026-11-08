import { Locator, Page, expect } from '@playwright/test';

/**
 * Shared interactions for the Optimizely Axiom (@optiaxiom/react) admin UI.
 *
 * Axiom dialogs render into a portal as role="dialog" named by their header, selects are
 * Downshift comboboxes (a button with role="combobox" plus a listbox of options), and toasts
 * stack, so these helpers lean on roles and accessible names rather than CSS classes.
 */

export function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

/** A dialog located by the text of its header, e.g. "Edit Source Directives". */
export function dialog(page: Page, title: string): Locator {
  return page.getByRole('dialog', { name: title, exact: true });
}

/**
 * A select trigger located by its field label. The combobox's accessible name is the
 * label followed by the currently selected option, so match on the label prefix.
 */
export function combobox(scope: Page | Locator, label: string): Locator {
  return scope.getByRole('combobox', { name: new RegExp(`^${escapeRegExp(label)}\\b`) });
}

/** The label of the option currently shown in a select trigger. */
export async function comboboxValue(trigger: Locator): Promise<string> {
  return (await trigger.innerText()).trim();
}

/**
 * Pick an option, by its visible label, from a select. No-op if already selected.
 * The listbox renders into a portal, so it is located from the page rather than the trigger.
 */
export async function selectOption(page: Page, trigger: Locator, optionLabel: string): Promise<void> {
  await expect(trigger).toBeEnabled();
  if (await comboboxValue(trigger) === optionLabel) {
    return;
  }

  await trigger.click();
  const listboxId = await trigger.getAttribute('aria-controls');
  const listbox = listboxId ? page.locator(`[id="${listboxId}"]`) : page.getByRole('listbox');
  await listbox.getByRole('option', { name: optionLabel, exact: true }).click();
  await expect(trigger).toHaveText(optionLabel);
}

/**
 * Assert a toast with the given text is showing. Toasts stack, so earlier toasts with
 * the same message may still be visible; any one of them satisfies the assertion.
 */
export async function expectToast(page: Page, text: string): Promise<void> {
  await expect(page.getByText(text, { exact: false }).first()).toBeVisible({ timeout: 10_000 });
}

/**
 * Wrapper around the shared ContextSwitcher component (Common/ContextSwitcher.jsx).
 */
export class ContextSwitcher {
  constructor(private readonly page: Page) {}

  /** The span showing the active context, immediately after the "Context:" label. */
  get label(): Locator {
    return this.page.getByText('Context:', { exact: true }).locator('xpath=following-sibling::span[1]');
  }

  get switchButton(): Locator {
    return this.page.getByRole('button', { name: 'Switch Context' });
  }

  /**
   * Open the context dialog, click the option with the given name and wait for the dialog
   * to close and the context label to update.
   *
   * @param optionName    Application name ("All Applications", "Test Website 1") or host display name ("https://localhost:5000/").
   * @param expectedLabel The value the context label shows afterwards ("All Applications", "TestWebsite1", "TestWebsite1 - localhost:5000").
   * @param beforeClick   Optional hook run just before the click, e.g. to start waiting for the scope's API
   *                      responses. Its result is awaited and returned once the context has switched.
   */
  async select<T>(optionName: string, expectedLabel: string, beforeClick?: () => Promise<T>): Promise<T | undefined> {
    await this.switchButton.click();

    const modal = dialog(this.page, 'Select Application Context');
    await expect(modal).toBeVisible();

    const option = modal.getByRole('option', { name: optionName, exact: true });
    await expect(option).toBeVisible();
    await option.scrollIntoViewIfNeeded();

    const pending = beforeClick?.();
    await option.click();

    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.label).toHaveText(expectedLabel);
    return pending ? await pending : undefined;
  }
}
