import './index.css'
import { StrictMode, Suspense } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter, Routes, Route } from "react-router"
import Layout from './Layout.tsx'
import App from './pages/App.tsx'
import PartitionCreate from './pages/PartitionCreate.tsx'
import Partition from './pages/Partition.tsx'
import PartitionIndex from './pages/PartitionIndex.tsx'
import PackageCreate from './pages/PackageCreate.tsx'
import Package from './pages/Package.tsx'
import PackageIndex from './pages/PackageIndex.tsx'
import ReleaseCreate from './pages/ReleaseCreate.tsx'
import Release from './pages/Release.tsx'
import ApplicationPoolCreate from './pages/ApplicationPoolCreate.tsx'
import ApplicationPool from './pages/ApplicationPool.tsx'
import {initializeIcons} from '@fluentui/font-icons-mdl2'
import editorWorker from 'monaco-editor/esm/vs/editor/editor.worker?worker'
import jsonWorker from 'monaco-editor/esm/vs/language/json/json.worker?worker'
import Site from './pages/Site.tsx'
import SiteCreate from './pages/SiteCreate.tsx'
import ScaleSets from './pages/ScaleSets.tsx'
import Instances from './pages/Instances.tsx'
import InstancesIndex from './pages/InstancesIndex.tsx'
import History from './pages/History.tsx'

self.MonacoEnvironment = {
  getWorker: function (_, label: string) {
    if (label === 'json') {
      return new jsonWorker()
    }
    return new editorWorker()
  },
}

initializeIcons();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Suspense fallback={'Loading...'}><Layout /></Suspense>}>
          <Route index element={<App />} />
          <Route path="/partitions/" element={<PartitionCreate />}></Route>
          <Route path="/partitions/:partitionName" element={<Partition />}>
            <Route index element={<PartitionIndex />} />
            <Route path="/partitions/:partitionName/apppools/" element={<ApplicationPoolCreate />} />
            <Route path="/partitions/:partitionName/apppools/:apppoolName" element={<ApplicationPool />} />
            <Route path="/partitions/:partitionName/sites/" element={<SiteCreate />} />
            <Route path="/partitions/:partitionName/sites/:siteName" element={<Site />} />
            <Route path="/partitions/:partitionName/scalesets/" element={<ScaleSets />} />
            <Route path="/partitions/:partitionName/instances/" element={<Instances />}>
              <Route index element={<InstancesIndex />} />
              <Route path="/partitions/:partitionName/instances/:scaleSetName/:instanceName" element={<History/>} />
            </Route>
          </Route>
          <Route path="/packages/" element={<PackageCreate />}></Route>
          <Route path="/packages/:packageName" element={<Package />}>
            <Route index element={<PackageIndex />} />
            <Route path="/packages/:packageName/releases" element={<ReleaseCreate />}></Route>
            <Route path="/packages/:packageName/releases/:versionName" element={<Release />}></Route>
          </Route>
        </Route>
      </Routes>
    </BrowserRouter>
  </StrictMode>
)
