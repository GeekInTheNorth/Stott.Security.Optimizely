// Helper for CSP Policy operations

const CspPolicyHelper = {
    /**
     * Returns a human-readable description of the policy's scope.
     * @param {Object} policy - A single CSP policy object.
     * @returns {string} Description of the policy's scope.
     */
    getScopeDescription(policy) {
        if (!policy) return 'No policy provided.';

        const {
            scopeType,
            scopeName,
            scopeBehavior,
            scopePaths,
            scopeExclusions
        } = policy;

        let desc = [];

        if (scopeType === 'Global') {
            desc.push('Applied globally');
        } else if (scopeType === 'Site') {
            desc.push('Applied for the site: ' + (scopeName || 'Unnamed Site'));
        }

        if (scopeBehavior === 'Any') {
            desc.push('for all route types');
        } else if (scopeBehavior === 'Content') {
            desc.push('for content routes');
        } else if (scopeBehavior === 'NonContent') {
            desc.push('for non-content routes');
        }

        if (Array.isArray(scopePaths) && (scopePaths.length > 1 || scopePaths[0] !== '/')) {
            desc.push('with specific paths');
        }

        if (Array.isArray(scopeExclusions) && scopeExclusions.length > 0) {
            desc.push('with specific exclusions');
        }

        return desc.length > 0 ? desc.join(", ") : "No scope information.";
    }
};

export default CspPolicyHelper;