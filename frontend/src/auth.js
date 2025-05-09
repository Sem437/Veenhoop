export const isAuthenticated = () => {
    const token = localStorage.getItem('token');
    return !!token; // true als er een token is
  };
  