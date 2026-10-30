import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Box, Button, Field, Flex, Switch, Text } from '@optiaxiom/react';
import { Range } from '@optiaxiom/react/unstable';
import { httpGet, httpPost } from '../Common/httpClient';
import OriginComponent from './OriginComponent';
import ExposeHeaderComponent from './ExposeHeaderComponent';
import AllowHeaderComponent from './AllowHeaderComponent';

function EditCorsSettings(props) {

    const [isCorsEnabled, setIsCorsEnabled] = useState(false);
    const [isAllowCredentials, setIsAllowCredentials] = useState(false);
    const [isAllowAllMethods, setIsAllowAllMethods] = useState(false);
    const [isAllowGetMethods, setIsAllowGetMethods] = useState(false);
    const [isAllowHeadMethods, setIsAllowHeadMethods] = useState(false);
    const [isAllowPostMethods, setIsAllowPostMethods] = useState(false);
    const [isAllowPutMethods, setIsAllowPutMethods] = useState(false);
    const [isAllowPatchMethods, setIsAllowPatchMethods] = useState(false);
    const [isAllowDeleteMethods, setIsAllowDeleteMethods] = useState(false);
    const [isAllowConnectMethods, setIsAllowConnectMethods] = useState(false);
    const [isAllowOptionsMethods, setIsAllowOptionsMethods] = useState(false);
    const [isAllowTraceMethods, setIsAllowTraceMethods] = useState(false);
    const [allowHeaders, setAllowHeaders] = useState([]);
    const [exposeHeaders, setExposeHeaders] = useState([]);
    const [allowedOrigins, setAllowOrigins] = useState([]);
    const [maxAgeParameter, setMaxAgeParameter] = useState(1);
    const [disableSaveButton, setDisableSaveButton] = useState(true);
    const [hasAllowHeadersError, setHasAllowHeadersError] =  useState(false);
    const [allowHeadersErrorMessage, setAllowHeadersErrorMessage] =  useState('');
    const [hasExposeHeadersError, setHasExposeHeadersError] =  useState(false);
    const [exposeHeadersErrorMessage, setExposeHeadersErrorMessage] =  useState('');
    const [hasAllowOriginsError, setHasAllowOriginsError] =  useState(false);
    const [allowOriginsErrorMessage, setAllowOriginsErrorMessage] =  useState('');
    const [hasAllowCredentialsError, setHasAllowCredentialsError] =  useState(false);
    const [allowCredentialsErrorMessage, setAllowCredentialsErrorMessage] =  useState('');

    useEffect(() => {
        getCorsSettings()
    }, [])

    const getCorsSettings = async () => {
        await httpGet(import.meta.env.VITE_CORS_GET)
            .then((response) => {
                setIsCorsEnabled(response.data.isEnabled ?? false);
                if (response.data.allowMethods?.isAllowAllMethods ?? false) {
                    setIsAllowAllMethods(response.data.allowMethods.isAllowAllMethods);
                }
                else {
                    setIsAllowAllMethods(false);
                    setIsAllowGetMethods(response.data.allowMethods?.isAllowGetMethods ?? false);
                    setIsAllowHeadMethods(response.data.allowMethods?.isAllowHeadMethods ?? false);
                    setIsAllowPostMethods(response.data.allowMethods?.isAllowPostMethods ?? false);
                    setIsAllowPutMethods(response.data.allowMethods?.isAllowPutMethods ?? false);
                    setIsAllowPatchMethods(response.data.allowMethods?.isAllowPatchMethods ?? false);
                    setIsAllowDeleteMethods(response.data.allowMethods?.isAllowDeleteMethods ?? false);
                    setIsAllowConnectMethods(response.data.allowMethods?.isAllowConnectMethods ?? false);
                    setIsAllowOptionsMethods(response.data.allowMethods?.isAllowOptionsMethods ?? false);
                    setIsAllowTraceMethods(response.data.allowMethods?.isAllowTraceMethods ?? false);
                }
        
                setAllowOrigins(response.data.allowOrigins ?? []);
                setAllowHeaders(response.data.allowHeaders ?? []);
                setExposeHeaders(response.data.exposeHeaders ?? []);
                setIsAllowCredentials(response.data.allowCredentials ?? false);
                setMaxAgeParameter(response.data.maxAge ?? 1)
                setDisableSaveButton(true);
            }, () => {
                handleShowFailureToast("Error", "Failed to retrieve CORS configuration. Please try again soon.")
            });
    }

    const getDisplayDuration = () => 
    {
        if (maxAgeParameter < 60) {
            return '(' + maxAgeParameter + ' seconds)';
        }
        else if (maxAgeParameter < 3600) {
            var minutesAndSeconds = new Date(maxAgeParameter * 1000).toISOString().substring(14, 19);
            return '(' + minutesAndSeconds + ' minutes)';
        }
        else {
            var hoursMinutesAndSeconds = new Date(maxAgeParameter * 1000).toISOString().substring(11, 19)
            return '(' + hoursMinutesAndSeconds + ' hours)';
        }
    }

    const handleIsCorsEnabledChange = (event) => {
        setIsCorsEnabled(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowCredentials = (event) => {
        setIsAllowCredentials(event.target.checked);
        setHasAllowCredentialsError(false);
        setAllowCredentialsErrorMessage('');
        setDisableSaveButton(false);        
    }

    const handleIsAllowAllMethods = (event) => {
        setIsAllowAllMethods(event.target.checked);
        setIsAllowGetMethods(isAllowGetMethods && !event.target.checked);
        setIsAllowHeadMethods(isAllowHeadMethods && !event.target.checked);
        setIsAllowPostMethods(isAllowPostMethods && !event.target.checked);
        setIsAllowPutMethods(isAllowPutMethods && !event.target.checked);
        setIsAllowPatchMethods(isAllowPatchMethods && !event.target.checked);
        setIsAllowDeleteMethods(isAllowDeleteMethods && !event.target.checked);
        setIsAllowConnectMethods(isAllowConnectMethods && !event.target.checked);
        setIsAllowOptionsMethods(isAllowOptionsMethods && !event.target.checked);
        setIsAllowTraceMethods(isAllowTraceMethods && !event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowGetMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowGetMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowHeadMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowHeadMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowPostMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowPostMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowPutMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowPutMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowPatchMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowPatchMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowDeleteMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowDeleteMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowConnectMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowConnectMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowOptionsMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowOptionsMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowTraceMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowTraceMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleSetMaxAgeParameter = (value) => 
    { 
        setMaxAgeParameter(value);
        setDisableSaveButton(false); 
    };

    const handleSaveAllowHeaders = (newHttpHeaders) => {
        setAllowHeaders(newHttpHeaders);
        setHasAllowHeadersError(false);
        setAllowHeadersErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleSaveExposeHeaders = (newHttpHeaders) => {
        setExposeHeaders(newHttpHeaders);
        setHasExposeHeadersError(false);
        setExposeHeadersErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleSaveAllowOrigins = (newAllowOrigins) => {
        setAllowOrigins(newAllowOrigins);
        setHasAllowOriginsError(false);
        setAllowOriginsErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleSaveSettings = async (event) => {
        event.preventDefault();

        let payload = {
            isEnabled: isCorsEnabled,
            allowOrigins: allowedOrigins,
            allowHeaders: allowHeaders,
            exposeHeaders: exposeHeaders,
            allowMethods: {
                isAllowGetMethods: isAllowAllMethods || isAllowGetMethods,
                isAllowHeadMethods: isAllowAllMethods || isAllowHeadMethods,
                isAllowPostMethods: isAllowAllMethods || isAllowPostMethods,
                isAllowPutMethods: isAllowAllMethods || isAllowPutMethods,
                isAllowPatchMethods: isAllowAllMethods || isAllowPatchMethods,
                isAllowDeleteMethods: isAllowAllMethods || isAllowDeleteMethods,
                isAllowConnectMethods: isAllowAllMethods || isAllowConnectMethods,
                isAllowOptionsMethods: isAllowAllMethods || isAllowOptionsMethods,
                isAllowTraceMethods: isAllowAllMethods || isAllowTraceMethods
            },
            allowCredentials: isAllowCredentials,
            maxAge: maxAgeParameter
        };

        await httpPost(import.meta.env.VITE_CORS_SAVE, payload)
            .then(() => {
                handleShowSuccessToast("CORS", "Changes to CORS settings have been successfully saved.");
            },
            (error) => {
                if (error.response && error.response.status === 400) {
                    var validationResult = error.response.data;
                    validationResult.errors.forEach(function (error) {
                        if (error.propertyName === 'AllowHeaders') {
                            setHasAllowHeadersError(true);
                            setAllowHeadersErrorMessage(error.errorMessage);
                        } else if (error.propertyName === 'ExposeHeaders') {
                            setHasExposeHeadersError(true);
                            setExposeHeadersErrorMessage(error.errorMessage);
                        } else if (error.propertyName === 'AllowOrigins') {
                            setHasAllowOriginsError(true);
                            setAllowOriginsErrorMessage(error.errorMessage);
                        } else if (error.propertyName === 'AllowCredentials') {
                            setHasAllowCredentialsError(true);
                            setAllowCredentialsErrorMessage(error.errorMessage);
                        }
                    });
                    handleShowFailureToast("CORS", "Changes to Cross-origin Resource Sharing settings have not been saved due to invalid values.");
                }
                else {
                    handleShowFailureToast("Error", "Failed to save changes to the Cross-origin Resource Sharing settings.");
                }
            });

        setDisableSaveButton(true);
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    return (
        <Box className='security-page-container'>
            <form>
                <Box my='16'>
                    <Switch checked={isCorsEnabled} onChange={handleIsCorsEnabledChange} description='Enabling the Cross-Origin Resource Sharing will apply the CORS headers to all requests from both content routes and CMS backend routes.'>Enable Cross-Origin Resource Sharing (CORS)</Switch>
                </Box>
                <OriginComponent origins={allowedOrigins} handleOriginUpdate={handleSaveAllowOrigins}></OriginComponent>
                {hasAllowOriginsError ? <Text color='fg.error' fontSize='sm'>{allowOriginsErrorMessage}</Text> : ""}
                <Box my='16'>
                    <Text id='lblAllowedHttpMethods' fontWeight='500' mb='8'>Allowed HTTP Methods:</Text>
                    <Flex flexDirection='column' alignItems='start' gap='8' mb='8'>
                        <Switch checked={isAllowAllMethods} onChange={handleIsAllowAllMethods}>Allow the use of ALL methods.</Switch>
                        <Switch checked={isAllowGetMethods} onChange={handleIsAllowGetMethods}>Allow the use of GET methods.</Switch>
                        <Switch checked={isAllowHeadMethods} onChange={handleIsAllowHeadMethods}>Allow the use of HEAD methods.</Switch>
                        <Switch checked={isAllowPostMethods} onChange={handleIsAllowPostMethods}>Allow the use of POST methods.</Switch>
                        <Switch checked={isAllowPutMethods} onChange={handleIsAllowPutMethods}>Allow the use of PUT methods.</Switch>
                        <Switch checked={isAllowPatchMethods} onChange={handleIsAllowPatchMethods}>Allow the use of PATCH methods.</Switch>
                        <Switch checked={isAllowDeleteMethods} onChange={handleIsAllowDeleteMethods}>Allow the use of DELETE methods.</Switch>
                        <Switch checked={isAllowConnectMethods} onChange={handleIsAllowConnectMethods}>Allow the use of CONNECT methods.</Switch>
                        <Switch checked={isAllowOptionsMethods} onChange={handleIsAllowOptionsMethods}>Allow the use of OPTIONS methods.</Switch>
                        <Switch checked={isAllowTraceMethods} onChange={handleIsAllowTraceMethods}>Allow the use of TRACE methods.</Switch>
                    </Flex>
                    <Text color='fg.tertiary' fontSize='sm'>Configures the 'Access-Control-Allow-Methods' header which instructs the browser on what HTTP Methods may be used when making a request to this webserver. If there are no method options selected, then the default behavior will be to allow ALL HTTP methods.</Text>
                </Box>
                <AllowHeaderComponent headers={allowHeaders} handleHeaderUpdate={handleSaveAllowHeaders}></AllowHeaderComponent>
                {hasAllowHeadersError ? <Text color='fg.error' fontSize='sm'>{allowHeadersErrorMessage}</Text> : ""}
                <ExposeHeaderComponent headers={exposeHeaders} handleHeaderUpdate={handleSaveExposeHeaders}></ExposeHeaderComponent>
                {hasExposeHeadersError ? <Text color='fg.error' fontSize='sm'>{exposeHeadersErrorMessage}</Text> : ""}
                <Box my='16'>
                    <Switch checked={isAllowCredentials} onChange={handleIsAllowCredentials} description="Configures the 'Access-Control-Allow-Credentials' header which instructs the browser whether it can share this request with the consuming website when the request's credential mode (Request.credentials) is set to 'include'.">Allow Credentials.</Switch>
                    {hasAllowCredentialsError ? <Text color='fg.error' fontSize='sm'>{allowCredentialsErrorMessage}</Text> : ""}
                </Box>
                <Field label={`Maximum Age ${getDisplayDuration()}`} description="Configures the 'Access-Control-Max-Age' header which instructs the browser on how long it may cache a pre-flight request." my='16'>
                    <Range min={1} max={7200} step={1} value={maxAgeParameter} onValueChange={handleSetMaxAgeParameter} />
                </Field>
                <Box my='16'>
                    <Button appearance='primary' type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Box>
            </form>
        </Box>
    )
}

EditCorsSettings.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default EditCorsSettings
