import { createRouter, createWebHistory } from 'vue-router'
// import DashboardView from '../views/DashboardView.vue'

const buildRoute = function (path, name, component) {
  return {
    path: path,
    name: name,
    component: component
  }
}

const buildViewComponent = function (url) {
  return () => import(/* webpackChunkName: "about" */ '../views/' + url)
}

const routes = [];

// routes.push(buildRoute('/', 'dashboard', DashboardView));
routes.push(buildRoute('/', 'dashboard', buildViewComponent('DashboardView.vue')));

// route level code-splitting
// this generates a separate chunk (about.[hash].js) for this route
// which is lazy-loaded when the route is visited.
routes.push(buildRoute('/about', 'about', buildViewComponent('AboutView.vue')));

// routes.push(buildRoute('/dashboard', 'dashboard', buildViewComponent('DashboardView.vue')));
routes.push(buildRoute('/dolar', 'dolar', buildViewComponent('DolarView.vue')));
routes.push(buildRoute('/fciRentaPesos', 'fciRentaPesos', buildViewComponent('FCIRentaPesosView.vue')));
routes.push(buildRoute('/fondos', 'fondos', buildViewComponent('FondosView.vue')));
routes.push(buildRoute('/iol', 'iol', buildViewComponent('IOLView.vue')));
routes.push(buildRoute('/lemon', 'lemon', buildViewComponent('LemonView.vue')));
routes.push(buildRoute('/mercadoPago', 'mercadoPago', buildViewComponent('MercadoPagoView.vue')));
routes.push(buildRoute('/plazosFijos', 'plazosFijos', buildViewComponent('PlazosFijosView.vue')));

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
