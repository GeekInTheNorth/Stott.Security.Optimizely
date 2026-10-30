import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Box, Button, Text } from '@optiaxiom/react';
import EditCorsHeader from './EditCorsHeader';

function AllowHeaderComponent (props) {

    const [httpHeaders, setHttpHeaders] = useState([]);

    const handleParentSave = (httpHeadersToSave) => props.handleHeaderUpdate && props.handleHeaderUpdate(httpHeadersToSave);

    const handleRemoveHttpHeader = (idToRemove) => {
        var newHttpHeaders = httpHeaders.filter(function (e) { return e.id !== idToRemove });
        setHttpHeaders(newHttpHeaders);
        handleParentSave(newHttpHeaders);
    };

    const handleUpdateHttpHeader = (headerId, newHeaderName) => {
        var newHttpHeaders = httpHeaders.map(x => x);
        newHttpHeaders.forEach(item => item.value = item.id === headerId ? newHeaderName : item.value)
        setHttpHeaders(newHttpHeaders);
        handleParentSave(newHttpHeaders);
    };

    const handleAddHttpHeader = () => {
        var newHttpHeaders = httpHeaders.map(x => x);
        newHttpHeaders.push({ id: crypto.randomUUID(), value: '' });
        setHttpHeaders(newHttpHeaders);
        handleParentSave(newHttpHeaders);
    }

    const renderAllowedHeaders = () => {
        return httpHeaders && httpHeaders.map((header) => {
            return (
                <EditCorsHeader key={header.id} headerId={header.id} headerName={header.value} handleDeleteHeader={handleRemoveHttpHeader} handleUpdateHeader={handleUpdateHttpHeader}></EditCorsHeader>
            )
        })
    }

    const setUpState = () => {
        setHttpHeaders(props.headers);
    }

    useEffect(() => {
        setUpState()
    }, [props.headers])

    return (
        <Box my='16'>
            <Text id='lblAllowedHttpHeaders' fontWeight='500' mb='8'>Allow Headers:</Text>
            <Box border='1' borderColor='border.default' rounded='md' p='16'>
                {renderAllowedHeaders()}
                <Button appearance='primary' type='button' onClick={handleAddHttpHeader} mb='16'>Add Header</Button>
                <Text color='fg.tertiary' fontSize='sm'>Configures the 'Access-Control-Allow-Headers' header which will be sent from this webserver to the browser to instruct the browser which headers that may be allowed within a web request. If no headers are provided here, then all headers will be considered as allowed.</Text>
                <Text color='fg.tertiary' fontSize='sm'>Please note that 'Accept', 'Accept-Language', 'Content-Language' and 'Content-Type' are considered as safe headers and do not need to be defined here.</Text>
            </Box>
        </Box>
    )
}

AllowHeaderComponent.propTypes = {
    headers: PropTypes.arrayOf(PropTypes.shape({
        id: PropTypes.string,
        value: PropTypes.string
    })).isRequired,
    handleHeaderUpdate: PropTypes.func
};

export default AllowHeaderComponent
