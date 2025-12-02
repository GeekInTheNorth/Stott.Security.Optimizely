import { createContext, useState, useCallback } from "react";
import axios from 'axios';

const StottSecurityContext = createContext();

export const StottSecurityProvider = ({ children, ...props }) => {

    const [permissionPolicySettings, setPermissionPolicySettings] = useState({ isEnabled: false });
    const [permissionPolicyCollection, setDirectiveCollection] = useState([]);
    const [permissionPolicySourceFilter, setPermissionPolicySourceFilter] = useState('');
    const [permissionPolicyDirectiveFilter, setPermissionPolicyDirectiveFilter] = useState('AllEnabled');

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
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
            await axios.get(import.meta.env.VITE_PERMISSION_POLICY_SOURCE_LIST, { params: { sourceFilter: sourceName, enabledFilter: directiveName } })
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

    const getPermissionPolicySettings = async () => {
        await axios.get(import.meta.env.VITE_PERMISSION_POLICY_SETTINGS_LOAD)
            .then((response) => {
                setPermissionPolicySettings(response.data);
            },
            () => {
                handleShowFailureToast("Error", "Failed to retrieve the Permissions Policy Settings.");
            });
    };

    const savePermissionPolicySettings = async (isEnabled) => {
        await axios.post(import.meta.env.VITE_PERMISSION_POLICY_SETTINGS_SAVE, { isEnabled: isEnabled })
            .then(() => {
                handleShowSuccessToast("Success", "Permissions Policy Settings have been successfully saved.");
                getPermissionPolicySettings();
            },
            () => {
                handleShowFailureToast("Error", "Failed to save the Permissions Policy Settings.");
            });
    };

    return (
        <StottSecurityContext.Provider value={
            {
                permissionPolicyCollection, 
                permissionPolicySourceFilter,
                permissionPolicyDirectiveFilter, 
                permissionPolicySettings, 
                setPermissionPolicySourceFilter, 
                setPermissionPolicyDirectiveFilter, 
                getPermissionPolicyDirectives, 
                getPermissionPolicySettings,
                savePermissionPolicySettings
            }}>
            {children}
        </StottSecurityContext.Provider>
    )
}

export { StottSecurityContext };
export default StottSecurityProvider;
