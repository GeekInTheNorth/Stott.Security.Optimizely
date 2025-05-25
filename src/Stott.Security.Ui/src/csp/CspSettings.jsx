import { } from 'react';
import PropTypes from 'prop-types';

function CspSettings({ cspPolicy }) {
  return (
    <>
      <label className='sso-formlabel' name='lblPolicyName'>Policy Name</label>
      <input className='sso-formcontrol' type="text" value={cspPolicy.name} aria-labelledby='lblPolicyName' />
    </>
  );
}

CspSettings.propTypes = {
  cspPolicy: PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    description: PropTypes.string,
    scopeType: PropTypes.string,
    scopeId: PropTypes.oneOfType([PropTypes.number, PropTypes.oneOf([null])]),
    scopeName: PropTypes.string,
    scopeBehavior: PropTypes.string,
    scopePaths: PropTypes.arrayOf(PropTypes.string),
    scopeExclusions: PropTypes.arrayOf(PropTypes.string),
    sources: PropTypes.arrayOf(
      PropTypes.shape({
        url: PropTypes.string.isRequired,
        directives: PropTypes.arrayOf(PropTypes.string).isRequired
      })
    )
  }).isRequired
};

export default CspSettings;