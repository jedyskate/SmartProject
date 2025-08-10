// src/routes.tsx
import type {ReactElement} from 'react';
import Home from './pages/Home';
import About from './pages/About';
import Orleans from './pages/Orleans/HelloWorld';
import SearchUserConfigs from "./pages/UserConfigs/Search/Index";
import EditUserConfig from "./pages/UserConfigs/Edit/Index.tsx";

interface RouteItem {
    path: string;
    element: ReactElement;
}

export const routes: RouteItem[] = [
    { path: '/', element: <Home /> },
    { path: '/about', element: <About /> },
    { path: '/orlenas/helloworld', element: <Orleans /> },
    { path: '/userconfigs/search/index', element: <SearchUserConfigs /> },
    { path: '/userconfigs/edit/:identifier', element: <EditUserConfig /> },
    { path: '/userconfigs/add', element: <EditUserConfig /> },
];
