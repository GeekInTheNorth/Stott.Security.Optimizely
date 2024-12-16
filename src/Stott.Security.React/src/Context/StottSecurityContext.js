import { createContext, useState, useCallback } from "react";
import axios from 'axios';

const StottSecurityContext = createContext();

export const StottSecurityProvider = ({ props, children }) => {

    const [permissionPolicyCollection, setDirectiveCollection] = useState([]);
    const [permissionPolicySourceFilter, setPermissionPolicySourceFilter] = useState('');
    const [permissionPolicyDirectiveFilter, setPermissionPolicyDirectiveFilter] = useState('AllEnabled');

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    // Debounce function
    const debounce = (func, delay) => {
        let debounceTimer;
        return function(...args) {
            const context = this;
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => func.apply(context, args), delay);
        };
    };

    const getPermissionPolicyDirectives = () => {
        getFilteredDirectives(permissionPolicySourceFilter, permissionPolicyDirectiveFilter);
    };

    const getFilteredDirectives = useCallback(
        debounce(async (sourceName, directiveName) => {
            await axios.get(process.env.REACT_APP_PERMISSION_POLICY_LIST, { params: { sourceFilter: sourceName, enabledFilter: directiveName } })
                .then((response) => {
                    if (Array.isArray(response.data)){
                        setDirectiveCollection(response.data);
                    }
                    else{
                        handleShowFailureToast("Get Permissions Policy Directives", "Failed to retrieve Permissions Policy Directives.");
                    }
                },
                () => {
                    handleShowFailureToast("Error", "Failed to retrieve the Permissions Policy Directives.");
                });
        }, 500),
        []
    );

    return (
        <StottSecurityContext.Provider value={{ permissionPolicyCollection, permissionPolicySourceFilter, permissionPolicyDirectiveFilter, setPermissionPolicySourceFilter, setPermissionPolicyDirectiveFilter, getPermissionPolicyDirectives }}>
            {children}
        </StottSecurityContext.Provider>
    )
}

export { StottSecurityContext };
export default StottSecurityProvider;