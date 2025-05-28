import { createContext, useContext, useState, useEffect } from "react";

// Create the context
const CspContext = createContext();

// Provider component
export function CspProvider({ children }) {
    const [allPolicies, setAllPolicies] = useState([]);
    const [viewMode, setViewMode] = useState('list');
    const [currentPolicy, setCurrentPolicy] = useState(null);

    useEffect(() => {
        if (import.meta.env.DEV) {
            fetch('/src/csp/data/csp-policies.mock.json')
                .then(res => res.json())
                .then(data => setAllPolicies(data));
        }
    }, []);

    const selectPolicy = (policy) => {
        if (import.meta.env.DEV) {
            console.log('Selected policy:', policy);
            let policyId = policy.id || 1; 
            let url = `/src/csp/data/csp-policy-${policyId}.mock.json`;

            fetch(url).then(res => res.json()).then(data => {
                setCurrentPolicy(data);
                setViewMode('settings');
            });
        }
    };

    const selectListMode = () => {
        setCurrentPolicy(null);
        setViewMode('list');
    };

    const selectPolicyView = (viewName) => {
        if (!currentPolicy) {
            console.warn(`Stott Security : No policy selected for view mode: ${viewName}`);
            selectListMode();
        } else {
            setViewMode(viewName);
        }
    }

    // You can add more state or methods as needed
    const value = {
        viewMode,
        selectListMode,
        selectPolicyView,
        selectPolicy,
        allPolicies,
        setAllPolicies,
        currentPolicy
    };

    return (
        <CspContext.Provider value={value}>
            {children}
        </CspContext.Provider>
    );
}

// Custom hook for easy usage
export function useCsp() {
    return useContext(CspContext);
}