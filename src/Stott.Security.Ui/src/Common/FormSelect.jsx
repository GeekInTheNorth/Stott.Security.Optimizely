import PropTypes from 'prop-types';
import { Select, SelectContent, SelectTrigger } from '@optiaxiom/react';

// Optiaxiom treats an empty string value as no selection, so empty values are swapped for a placeholder value.
const EmptyValue = '__empty__';

const toSelectValue = (value) => (value === null || value === undefined || value === '') ? EmptyValue : String(value);
const fromSelectValue = (value) => value === EmptyValue ? '' : value;

function FormSelect({ options, value, onValueChange, disabled, ...triggerProps }) {

    const selectOptions = options.map((option) => ({ label: option.label, value: toSelectValue(option.value) }));

    const handleValueChange = (selectedValue) => onValueChange && onValueChange(fromSelectValue(selectedValue));

    return (
        <Select options={selectOptions} value={toSelectValue(value)} onValueChange={handleValueChange} disabled={disabled}>
            <SelectTrigger w='full' {...triggerProps} />
            <SelectContent />
        </Select>
    );
}

FormSelect.propTypes = {
    options: PropTypes.arrayOf(PropTypes.shape({
        label: PropTypes.string.isRequired,
        value: PropTypes.oneOfType([PropTypes.string, PropTypes.number, PropTypes.bool])
    })).isRequired,
    value: PropTypes.oneOfType([PropTypes.string, PropTypes.number, PropTypes.bool]),
    onValueChange: PropTypes.func,
    disabled: PropTypes.bool
};

export default FormSelect;
