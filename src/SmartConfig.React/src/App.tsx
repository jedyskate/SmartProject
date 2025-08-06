import { Suspense } from 'react'
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { routes } from './routes.js';
import './App.css'

// const Home = lazy(() => import('./pages/Home'));
// const About = lazy(() => import('./pages/About'));

function App() {
    return (
        <BrowserRouter>
            <nav style={{
                position: 'absolute',
                top: '1rem',
                right: '1rem',
                display: 'flex',
                gap: '1rem'
            }}>
                <Link to="/">Home</Link>
                <Link to="/about">About</Link>
            </nav>
            
            {/* Routing */}
            <Suspense fallback={<div>Loading...</div>}>
                <Routes>
                    {routes.map((route, i) => (
                        <Route key={i} path={route.path} element={route.element} />
                    ))}
                </Routes>
            </Suspense>

        </BrowserRouter>
    )
}

export default App
