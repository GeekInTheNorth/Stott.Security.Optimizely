import { createRoot } from 'react-dom/client';
import NavigationContainer from './Navigation/NavigationContainer';
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';

const domNode = document.getElementById('security-app');
const root = createRoot(domNode);

root.render(<NavigationContainer />);