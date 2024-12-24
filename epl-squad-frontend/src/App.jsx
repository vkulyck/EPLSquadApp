import React, { useState } from 'react';
import SquadSearch from './components/SquadSearch';
import SquadDetails from './components/SquadDetails';
import FootballApiService from './services/FootballApiService';
import './App.css';

const App = () => {
    const [squad, setSquad] = useState([]);
    const [error, setError] = useState('');
    const [teamName, setTeamName] = useState(''); // State to store the real team name
    const [crest, setCrest] = useState(''); // State to store the team crest URL

    const fetchSquad = async (teamNameInput, season) => {
        try {
            setError(''); // Clear any previous error
            const data = await FootballApiService.getTeamSquad(teamNameInput, season);
            setTeamName(data.name); // Update team name
            setSquad(data.squad || []); // Update squad
            setCrest(data.crest); // Update crest
        } catch (err) {
            setError(err.message); // Set the user-friendly error message
            setSquad([]); // Clear the squad data
        }
    };

    return (
        <div>
            <h1>English Premier League Squad Finder</h1>
            <SquadSearch onSearch={fetchSquad} />
            <SquadDetails squad={squad} error={error} teamName={teamName} crest={crest} />
        </div>
    );
};

export default App;