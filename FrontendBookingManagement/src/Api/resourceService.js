import api from "./axiosConfig";

export const resourceService = {
  getAllResources: async () => {
    const response = await api.get("/Resources");
    console.log("true");

    return response.data;
  },

  getResourceById: async (id) => {
    const response = await api.get(`/Resources/${id}`);
    return response.data;
  },

  createResource: async (resourceData) => {
    const response = await api.post("/Resources", resourceData);
    return response.data;
  },
};
