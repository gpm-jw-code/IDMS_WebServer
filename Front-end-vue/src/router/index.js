import { createRouter, createWebHistory } from 'vue-router'
import ComponetTestVue from '@/views/ComponentTestPage'
// import LogView from '../components/LogView.vue'
import AdminView from '../views/AdminView.vue'
import IDMS from '@/components/IDMS/MainPage.vue'
import ModuleStatesListViewVue from '@/components/IDMS/ModuleStatesListView.vue'
import QueryPage from '@/components/IDMS/QueryPage.vue'
import { configs } from '@/config'
import EntryPage from '@/components/IDMS/EntryPage/EntryPage.vue'
import VEPage from "@/components/IDMS/VbEnergyMonitor/VbEnergyView.vue"


var gpm_mode_routes = [

  {
    path: '/',
    name: 'EntryPage',
    component: EntryPage,
  },
  {
    path: '/EdgeMain',
    name: '診斷頁面',
    component: IDMS,
  },
  {
    path: '/ve',
    name: '振動能量',
    component: VEPage,
  },
  {
    path: '/modulestates',
    name: '感測器狀態',
    component: ModuleStatesListViewVue,
  },
  {
    path: '/query',
    name: 'Query',
    component: QueryPage,
  }
]


const routes = gpm_mode_routes

const router = createRouter({
  history: createWebHistory(),
  routes,
})

/**
 * 取得所有路由 name 屬性
 */
export function GetRouters() {
  return routes
}

export default router
