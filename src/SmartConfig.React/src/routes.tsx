// src/routes.tsx
import type {ReactElement} from 'react';
import Home from './pages/Home';
import About from './pages/About';
import Orleans from './pages/Orleans/HelloWorld';

interface RouteItem {
    path: string;
    element: ReactElement;
}

export const routes: RouteItem[] = [
    { path: '/', element: <Home /> },
    { path: '/about', element: <About /> },
    { path: '/orlenas/hello-world', element: <Orleans /> },
];
