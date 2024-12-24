import axios from 'axios';
import config from '../config.json';

const API_BASE_URL = config.API_BASE_URL;

class FootballApiService {
    static async getTeamSquad(teamName, season = null) {
        try {
            const response = await axios.get(`${API_BASE_URL}/teams/${teamName}`, {
                params: season ? { season } : {},
            });

            const data = response.data;

            return {
                name: data.name,
                crest: data.crest,
                squad: data.squad,
            };
        } catch (error) {
            // Extract the error message from the backend response
            const errorMessage = error.response?.data?.message || error.message || 'An unknown error occurred.';
            throw new Error(errorMessage);
        }
    }
}

export default FootballApiService;
