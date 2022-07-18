<template>
  <nav>
    <router-link to="/">Dashboard</router-link> |
    <router-link to="/fondos">Fondos</router-link> |
    <router-link to="/fciRentaPesos">FCI FBA Renta Pesos</router-link> |
    <router-link to="/plazosFijos">Plazos Fijos</router-link> |
    <router-link to="/mercadoPago">Mercado Pago</router-link> |
    <router-link to="/lemon">Lemon</router-link> |
    <router-link to="/dolar">Dólar</router-link> |
    <router-link to="/iol">IOL</router-link>
  </nav>
  <router-view/>
</template>

<style>
body {
  padding: 0!important;
  margin: 0!important;
}

#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

nav {
  padding: 10px;
  background-color: maroon;
}

nav a {
  font-weight: bold;
  color: #2c3e50;
  text-decoration: none;
  background-color: red;
  padding: 8px;
}

nav a.router-link-exact-active {
  color: #42b983;
}
</style>
