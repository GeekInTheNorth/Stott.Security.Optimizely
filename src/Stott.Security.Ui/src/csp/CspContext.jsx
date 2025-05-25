import React, { createContext, useContext, useState, useEffect } from "react";

// Create the context
const CspContext = createContext();

// Provider component
export function CspProvider({ children }) {
    const [allPolicies, setAllPolicies] = useState([]);
    const [currentPolicy, setCurrentPolicy] = useState(null);
    const [currentPolicySources, setCurrentPolicySources] = useState([]);

    useEffect(() => {
        if (import.meta.env.DEV) {
            fetch('/src/csp/data/csp-policies.mock.json')
                .then(res => res.json())
                .then(data => setAllPolicies(data));
        }
    }, []);

    // You can add more state or methods as needed
    const value = {
        allPolicies,
        setAllPolicies,
        currentPolicy,
        setCurrentPolicy,
        currentPolicySources,
        setCurrentPolicySources,
        addCspSource: item => setCurrentPolicySources(items => [...items, item]),
        removeCspSource: idx => setCurrentPolicySources(items => items.filter((_, i) => i !== idx))
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