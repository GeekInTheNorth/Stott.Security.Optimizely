import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Form } from 'react-bootstrap';

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

    const handleMaxAgeDaysChange = (event) => {
        const newMaxAgeDays = parseInt(event.target.value);
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
        <div className='border rounded p-3 bg-light'>
            <Form.Group className='mb-3'>
                <Form.Label>Max Age: {formatDaysLabel(maxAgeDays)}</Form.Label>
                <Form.Range min={1} max={730} step={1} value={maxAgeDays} onChange={handleMaxAgeDaysChange} />
                <div className='d-flex justify-content-between'>
                    <small className='text-muted'>1 day</small>
                    <small className='text-muted'>2 years</small>
                </div>
            </Form.Group>
            <Form.Group className='mb-2'>
                <Form.Check type='checkbox' id='hstsIncludeSubDomains' label='Include Sub Domains' checked={includeSubDomains} onChange={handleIncludeSubDomainsChange} />
                <Form.Text className='text-muted'>Apply HSTS policy to all subdomains.</Form.Text>
            </Form.Group>
            <Form.Group className='mb-2'>
                <Form.Check type='checkbox' id='hstsPreload' label='Preload' checked={preload} onChange={handlePreloadChange} />
                <Form.Text className='text-muted'>Allow the domain to be included in browser HSTS preload lists.</Form.Text>
            </Form.Group>
            <Form.Group className='mt-3'>
                <Form.Label>Generated Header Value</Form.Label>
                <Form.Control type='text' value={props.value || ''} readOnly className='bg-white' />
            </Form.Group>
        </div>
    );
}

HstsHeaderValue.propTypes = {
    value: PropTypes.string,
    onChange: PropTypes.func
};

export default HstsHeaderValue;
