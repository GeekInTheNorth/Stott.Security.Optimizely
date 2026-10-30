import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Box, Checkbox, Field, Input } from '@optiaxiom/react';
import { Range } from '@optiaxiom/react/unstable';

function HstsHeaderValue(props) {
    const [maxAgeDays, setMaxAgeDays] = useState(365);
    const [includeSubDomains, setIncludeSubDomains] = useState(false);
    const [preload, setPreload] = useState(false);

    const SECONDS_PER_DAY = 86400;

    const parseHstsValue = (value) => {
        if (!value) {
            return { maxAgeDays: 365, includeSubDomains: false, preload: false };
        }

        let parsedMaxAgeDays = 365;
        let parsedIncludeSubDomains = false;
        let parsedPreload = false;

        const maxAgeMatch = value.match(/max-age=(\d+)/i);
        if (maxAgeMatch) {
            parsedMaxAgeDays = Math.round(parseInt(maxAgeMatch[1]) / SECONDS_PER_DAY);
            parsedMaxAgeDays = Math.max(1, Math.min(730, parsedMaxAgeDays));
        }

        parsedIncludeSubDomains = /includeSubDomains/i.test(value);
        parsedPreload = /preload/i.test(value);

        return { maxAgeDays: parsedMaxAgeDays, includeSubDomains: parsedIncludeSubDomains, preload: parsedPreload };
    };

    const buildHstsValue = (days, subDomains, preloadFlag) => {
        const parts = [`max-age=${days * SECONDS_PER_DAY}`];
        if (subDomains) {
            parts.push('includeSubDomains');
        }
        if (preloadFlag) {
            parts.push('preload');
        }
        return parts.join('; ');
    };

    useEffect(() => {
        const hstsValues = parseHstsValue(props.value);
        setMaxAgeDays(hstsValues.maxAgeDays);
        setIncludeSubDomains(hstsValues.includeSubDomains);
        setPreload(hstsValues.preload);
    }, [props.value]);

    const handleMaxAgeDaysChange = (value) => {
        const newMaxAgeDays = value;
        setMaxAgeDays(newMaxAgeDays);
        const newValue = buildHstsValue(newMaxAgeDays, includeSubDomains, preload);
        props.onChange && props.onChange(newValue);
    };

    const handleIncludeSubDomainsChange = (event) => {
        const newIncludeSubDomains = event.target.checked;
        setIncludeSubDomains(newIncludeSubDomains);
        const newValue = buildHstsValue(maxAgeDays, newIncludeSubDomains, preload);
        props.onChange && props.onChange(newValue);
    };

    const handlePreloadChange = (event) => {
        const newPreload = event.target.checked;
        setPreload(newPreload);
        const newValue = buildHstsValue(maxAgeDays, includeSubDomains, newPreload);
        props.onChange && props.onChange(newValue);
    };

    const formatDaysLabel = (days) => {
        if (days < 30) {
            return `${days} day${days === 1 ? '' : 's'}`;
        } else if (days < 365) {
            const months = Math.round(days / 30);
            return `${months} month${months === 1 ? '' : 's'}`;
        } else {
            const years = (days / 365).toFixed(1);
            return `${years} year${years === '1.0' ? '' : 's'}`;
        }
    };

    return (
        <Box border='1' borderColor='border.default' rounded='md' p='16' bg='bg.secondary'>
            <Field label={`Max Age: ${formatDaysLabel(maxAgeDays)}`} mb='16'>
                <Range min={1} max={730} step={1} value={maxAgeDays} onValueChange={handleMaxAgeDaysChange} marks={[{ value: 1, label: '1 day' }, { value: 730, label: '2 years' }]} />
            </Field>
            <Checkbox id='hstsIncludeSubDomains' checked={includeSubDomains} onChange={handleIncludeSubDomainsChange} description='Apply HSTS policy to all subdomains.' mb='8'>Include Sub Domains</Checkbox>
            <Checkbox id='hstsPreload' checked={preload} onChange={handlePreloadChange} description='Allow the domain to be included in browser HSTS preload lists.' mb='8'>Preload</Checkbox>
            <Field label='Generated Header Value' mt='16'>
                <Input type='text' value={props.value || ''} readOnly />
            </Field>
        </Box>
    );
}

HstsHeaderValue.propTypes = {
    value: PropTypes.string,
    onChange: PropTypes.func
};

export default HstsHeaderValue;
