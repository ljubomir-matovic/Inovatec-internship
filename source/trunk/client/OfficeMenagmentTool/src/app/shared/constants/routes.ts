export const AppRoutes = {
    shared: {
        categoriesAdministrationRoute: 'categories-administration',
        equipmentAdministrationRoute: 'equipment-administration',
        equipmentEvidentionRoute: 'equipment-evidention',
        reportProblemRoute: 'report-problem',
        reportedProblemsRoute: 'reported-problems',
        myProblemsRoute: 'my-problems',
        orderSnacksRoute: 'order-snacks',
        snackOrdersRoute: 'snack-orders',
        snacksAdministrationRoute: 'snack-administration',
        editProfileRoute: 'edit-profile',
        changePasswordRoute: 'change-password',
        myEquipmentsRoute: 'my-equipments',
        ordersHistoryRoute: 'orders-history',
        orderEquipment: 'order-equipment',
        equipmentOrders: 'equipment-orders',
        myEquipmentOrders: 'my-equipment-orders',
        officeAdministrationRoute: 'office-administration'
    },
    user: {
        defaultRoute: 'my-problems',
    },
    hr: {
        defaultRoute: 'categories-administration',
        groupRoute: 'categories-administration',
        suppliersAdministrationRoute: 'suppliers-administration',
        acceptOrder: 'accept-order',
        ordersListingRoute: 'orders',
        reportSchedulesRoute: 'reports-schedules',
        orderRoute: 'order/:id'
    },
    admin: {
        defaultRoute: 'admin-dashboard',
        adminDashboard: 'admin-dashboard',
        createUser: 'create-user',
        logsAdministration: 'logs-administration'
    }
}
