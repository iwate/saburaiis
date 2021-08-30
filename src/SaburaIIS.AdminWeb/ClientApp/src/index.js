import './index.css';
import React, { Suspense } from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, Route, Switch } from "react-router-dom";

import App from './pages/App';
import Partition from './pages/Partition';
import Layout from './shared/Layout';
import reportWebVitals from './reportWebVitals';

import {initializeIcons} from '@fluentui/font-icons-mdl2'
import { RecoilRoot } from 'recoil';
import { PartitionCreate } from './pages/PartitionCreate';
import { PackageCreate } from './pages/PackageCreate';
import { Package } from './pages/Package';

initializeIcons();

ReactDOM.render(
  <React.StrictMode>
    <RecoilRoot>
      <Router>
        <Suspense fallback="Loading...">
          <Layout>
            <Switch>
              <Route path="/" component={App} exact></Route>
              <Route path="/partitions/:partitionName" component={Partition}></Route>
              <Route path="/partitions/" component={PartitionCreate}></Route>
              <Route path="/packages/:packageName" component={Package}></Route>
              <Route path="/packages/" component={PackageCreate}></Route>
            </Switch>
          </Layout>
        </Suspense>
      </Router>
    </RecoilRoot>
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
