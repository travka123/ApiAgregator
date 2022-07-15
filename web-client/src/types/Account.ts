interface Account {
    jwt: string;
    login: string;
    email: string;
    emailConfirmed: boolean;
    isAdmin: boolean;
}

export default Account;