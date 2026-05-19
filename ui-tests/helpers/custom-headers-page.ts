import { Page, expect } from '@playwright/test';

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

interface ApplicationApiResponse {
  appId: string | null;
  appName: string;
  availableHosts?: Array<{ hostName?: string; displayName?: string }>;
  hasMultipleHosts?: boolean;
}

export class CustomHeadersPage {
  constructor(private readonly page: Page, private readonly cmsUrl: string) {}

  async open(): Promise<void> {
    await this.page.goto(`${this.cmsUrl}/stott.security.optimizely/administration/#response-headers`);
    await expect(this.page.getByRole('button', { name: 'Switch Context' })).toBeVisible();
  }

  private async openContextModal() {
    await this.page.getByRole('button', { name: 'Switch Context' }).click();
    const modal = this.page.locator('.modal.show', { hasText: 'Select Application Context' });
    await expect(modal).toBeVisible();
    return modal;
  }

  async switchToGlobal(): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal.locator('.list-group-item', {
      has: this.page.locator('strong', { hasText: /^All Applications$/ }),
    }).first();
    await expect(row).toBeVisible();
    await row.scrollIntoViewIfNeeded();
    await row.click();

    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText('All Applications');
  }

  async switchToApplication(appDisplayName: string, expectedAppId: string): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal.locator('.list-group-item', {
      has: this.page.locator('strong', { hasText: new RegExp(`^${escapeRegExp(appDisplayName)}$`) }),
    }).first();
    await expect(row).toBeVisible();
    await row.scrollIntoViewIfNeeded();
    await row.click();

    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(expectedAppId);
  }

  async switchToHost(hostDisplayName: string, expectedAppId: string, expectedHostName: string): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal
      .locator('.list-group-item', { hasText: hostDisplayName })
      .filter({ hasText: 'Host-level configuration' })
      .first();
    await expect(row).toBeVisible();
    await row.scrollIntoViewIfNeeded();
    await row.click();

    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(`${expectedAppId} - ${expectedHostName}`);
  }

  /**
   * Clicks "Create Override" when the inherited alert is showing. No-op if the
   * current scope is already overridden (or is the global scope, which has no
   * override concept).
   */
  async ensureOverrideExists(): Promise<void> {
    const createOverride = this.page.getByRole('button', { name: 'Create Override' });
    if (await createOverride.isVisible()) {
      await createOverride.click();
      await expect(this.page.getByRole('button', { name: 'Revert to Inherited' })).toBeVisible({ timeout: 10_000 });
    }
  }

  /**
   * If the current scope has its own override, click "Revert to Inherited" to
   * delete it. Returns true if a revert was performed.
   */
  async revertToInheritedIfPresent(): Promise<boolean> {
    const revert = this.page.getByRole('button', { name: 'Revert to Inherited' });
    if (await revert.isVisible()) {
      await revert.click();
      await expect(this.page.getByRole('button', { name: 'Create Override' })).toBeVisible({ timeout: 10_000 });
      return true;
    }
    return false;
  }

  /**
   * Open the Add Custom Header modal, set behavior=Add (the default), fill the
   * name and value fields, save, and wait for the success toast.
   */
  async addHeader(headerName: string, headerValue: string): Promise<void> {
    await this.page.getByRole('button', { name: 'Add Header' }).click();

    const modal = this.page.locator('.modal.show', { hasText: 'Add Custom Header' });
    await expect(modal).toBeVisible();

    // Header Name input — placeholder identifies it uniquely within the modal.
    await modal.locator('input[placeholder*="X-Permitted"]').fill(headerName);

    // Behavior defaults to 1 (Add) for new headers, no change required. The
    // value input only renders while behavior === 1.
    await modal.locator('input[placeholder="e.g., none"]').fill(headerValue);

    await modal.getByRole('button', { name: 'Save' }).click();
    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.getByText('Custom header has been successfully saved.', { exact: false })).toBeVisible({ timeout: 10_000 });
  }

  /**
   * Click Delete on the card whose header name matches, then confirm in the
   * generic ConfirmationModal.
   */
  async deleteHeader(headerName: string): Promise<void> {
    const card = this.page.locator('.card', {
      has: this.page.locator('.card-header', { hasText: new RegExp(`^${escapeRegExp(headerName)}$`) }),
    }).first();
    await expect(card).toBeVisible();
    await card.getByRole('button', { name: 'Delete' }).click();

    const confirmModal = this.page.locator('.modal.show', { hasText: 'Delete Header' });
    await expect(confirmModal).toBeVisible();
    await confirmModal.getByRole('button', { name: 'Delete' }).click();

    await expect(confirmModal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('.card', {
      has: this.page.locator('.card-header', { hasText: new RegExp(`^${escapeRegExp(headerName)}$`) }),
    })).toHaveCount(0, { timeout: 10_000 });
  }

  /**
   * Walk every application + host scope exposed by the applications API and
   * revert any custom-header override that exists. Mirrors the equivalent
   * helper on PermissionsPolicyPage — driven from the live API so we only
   * visit scopes that genuinely exist in the seeded sites.
   */
  async revertAllOverrides(): Promise<void> {
    const url = `${this.cmsUrl}/stott.security.optimizely/api/applications`;
    const response = await this.page.request.get(url, { ignoreHTTPSErrors: true });
    if (!response.ok()) {
      throw new Error(`Failed to fetch applications list (${response.status()}): ${url}`);
    }
    const apps: ApplicationApiResponse[] = await response.json();

    for (const app of apps) {
      if (!app.appId) continue; // skip the synthetic "All Applications" entry
      await this.switchToApplication(app.appName, app.appId);
      await this.revertToInheritedIfPresent();

      if (app.hasMultipleHosts && app.availableHosts) {
        for (const host of app.availableHosts) {
          if (!host.hostName || !host.displayName) continue; // skip the "Default" placeholder
          await this.switchToHost(host.displayName, app.appId, host.hostName);
          await this.revertToInheritedIfPresent();
        }
      }
    }

    await this.switchToGlobal();
  }
}
