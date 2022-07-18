import React from 'react';
import { Route, Routes } from 'react-router-dom';
import { Authentication } from './components/Authentication';
import EmailConfirmation from './components/EmailConfirmation';
import EmailConfirmationHandler from './components/EmailConfirmationHandler';
import AdminPage from './pages/AdminPage';
import EmailConfirmationPage from './pages/EmailConfirmationPage';
import Page from './pages/Page';
import SignInPage from './pages/SignInPage';
import SignUnPage from './pages/SignUpPage';
import TaskPage from './pages/TaskPage';

const App: React.FC = () => {
  return (
    <div className="App">
      <Routes>

        <Route index element={<Authentication redirect={<SignInPage />}
          children={<EmailConfirmation redirect={<EmailConfirmationPage />}
            children={<Page children={<TaskPage />} />} />} />} />

        <Route path='/admin' element={<Authentication redirect={<SignInPage />}
          children={<EmailConfirmation redirect={<EmailConfirmationPage />}
            children={<Page children={<AdminPage />} />} />} />} />

        <Route path='/signin' element={<Authentication redirect={<SignInPage />}
          children={"already authorized"} />} />

        <Route path='/signup' element={<Authentication redirect={<SignUnPage />}
          children={"already registered"} />} />

        <Route path='/confirm'>
          <Route path=':token' element={<Authentication redirect={<SignInPage />}
            children={<EmailConfirmation redirect={<EmailConfirmationHandler />}
              children={"already confirmed"} />} />} />
        </Route>

      </Routes>
    </div>
  );
}

export default App;
