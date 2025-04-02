import { ApolloClient, InMemoryCache, HttpLink } from '@apollo/client';

const client = new ApolloClient({
    link: new HttpLink({
        uri: 'http://localhost:8080/graphql', // Replace with your backend URL
        credentials: 'include',
    }),
    cache: new InMemoryCache(),
});

export default client;