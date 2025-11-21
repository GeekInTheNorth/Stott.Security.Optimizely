import { createContext, useContext, useState, useEffect } from "react";
import axios from "axios";

// Create the context
const CspContext = createContext();

// Provider component
export function CspProvider({ children }) {
    const [allPolicies, setAllPolicies] = useState([]);
    const [allSites, setAllSites] = useState([]);
    const [viewMode, setViewMode] = useState('list');
    const [currentPolicy, setCurrentPolicy] = useState(null);

    useEffect(() => {
        fetchPolicies();
        fetchSites();
    }, []);

    const fetchPolicies = () => {
        axios.get(import.meta.env.VITE_APP_CSP_SETTINGS_LIST)
            .then(response => {
                setAllPolicies(response.data);
            })
            .catch(error => {
                setAllPolicies([]);
                console.error('Error fetching policies:', error);
            });
    }

    const fetchSites = () => {
        axios.get(import.meta.env.VITE_APP_CMS_SITES_LIST)
            .then(response => {
                setAllSites(response.data);
            })
            .catch(error => {
                setAllSites([]);
                console.error('Error fetching sites:', error);
            });
    }

    const selectPolicy = (policy, mode) => {
        if (policy && policy.id) {
            axios.get(import.meta.env.VITE_APP_CSP_SETTINGS_GET, { params: { id: policy.id } })
                .then(response => {
                    setCurrentPolicy(response.data);
                    setViewMode(mode || 'settings');
                })
                .catch(error => {
                    console.error(`Error fetching policy by name: ${error}`);
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
        currentPolicy,
        allSites,
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