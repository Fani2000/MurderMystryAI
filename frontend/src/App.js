import React, { useState } from 'react';
import { useMutation, gql } from '@apollo/client';
import {
  Box,
  Button,
  Typography,
  TextField,
  List,
  ListItem,
  Paper,
  Divider,
} from '@mui/material';
import client from './apollo-client';

// GraphQL mutation for `GenerateMurderMysteryAsync`
const GENERATE_MURDER_MYSTERY = gql`
  mutation GenerateMurderMystery($users: [UserInput!]!) {
    generateMurderMystery(users: $users) {
      fullStory
      killer
      detective
      detectiveClues
      userStories {
        key
        value
      }
    }
  }
`;

function App() {
  const [participants, setParticipants] = useState([]);
  const [name, setName] = useState('');
  const [generatedStory, setGeneratedStory] = useState(null);

  const [generateMystery, { loading, error }] = useMutation(
    GENERATE_MURDER_MYSTERY,
    { client }
  );

  const handleAddParticipant = () => {
    if (name.trim()) {
      setParticipants([...participants, { name }]);
      setName('');
    }
  };

  const handleGenerate = async () => {
    try {
      const { data } = await generateMystery({
        variables: { users: participants },
      });
      console.log('Generated story:', data.generateMurderMystery);
      setGeneratedStory(data.generateMurderMystery);
    } catch (err) {
      console.error('Error generating story:', err);
    }
  };

  return (
    <Box sx={{ padding: 4 }}>
      {/* Title */}
      <Typography variant="h4" gutterBottom>
        Murder Mystery Generator
      </Typography>

      {/* Input and Story Output Section */}
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'flex-start',
          justifyContent: 'space-between',
          gap: 4,
          marginTop: 4,
        }}
      >
        {/* Left Column: Add Participants */}
        <Box
          sx={{
            flex: 1,
            display: 'flex',
            flexDirection: 'column',
            gap: 2,
          }}
        >
          {/* Add Participants Section */}
          <Box>
            <Typography variant="h6" gutterBottom>
              Add Participants
            </Typography>
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                alignItems: 'center',
                gap: 2,
              }}
            >
              <TextField
                label="Participant Name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                onKeyDown={(e) => (e.key === 'Enter' ? handleAddParticipant() : null)}
              />
              <Button variant="contained" onClick={handleAddParticipant}>
                Add
              </Button>
            </Box>
          </Box>

          {/* Participant List */}
          <List sx={{ marginTop: 2 }}>
            {participants.map((participant, index) => (
              <ListItem
                key={index}
                sx={{
                  padding: 1,
                  backgroundColor: '#f5f5f5',
                  marginBottom: 1,
                  borderRadius: 1,
                }}
              >
                {participant.name}
              </ListItem>
            ))}
          </List>

          {/* Generate Mystery Button */}
          <Button
            variant="contained"
            color="primary"
            onClick={handleGenerate}
            disabled={loading || participants.length < 3}
          >
            {loading ? 'Generating Mystery...' : 'Generate Mystery'}
          </Button>

          {error && (
            <Typography color="error" sx={{ marginTop: 2 }}>
              Error generating mystery. Please try again.
            </Typography>
          )}
        </Box>

        {/* Right Column: Generated Story */}
        {generatedStory && (
          <Box sx={{ flex: 2 }}>
            <Typography variant="h6" gutterBottom>
              Murder Mystery Story
            </Typography>
            <Paper sx={{ padding: 2, marginBottom: 2 }}>
              <Typography variant="subtitle1" gutterBottom>
                <strong>Full Story:</strong>
              </Typography>
              <Typography>{generatedStory.fullStory}</Typography>
            </Paper>

            <Divider sx={{ marginY: 2 }} />

            <Typography variant="subtitle1" gutterBottom>
              <strong>Killer:</strong> {generatedStory.killer}
            </Typography>
            <Typography variant="subtitle1" gutterBottom>
              <strong>Detective:</strong> {generatedStory.detective}
            </Typography>

            {generatedStory.detectiveClues && (
              <Box
                sx={{
                  marginTop: 2,
                  padding: 2,
                  backgroundColor: '#e0f7fa',
                  borderRadius: 1,
                }}
              >
                <Typography variant="subtitle1" gutterBottom>
                  <strong>Detective Clues:</strong>
                </Typography>
                <ul>
                  {generatedStory.detectiveClues.map((clue, index) => (
                    <li key={index}>
                      <Typography>{clue}</Typography>
                    </li>
                  ))}
                </ul>
              </Box>
            )}

            <Divider sx={{ marginY: 2 }} />

            {/* User-Specific Stories */}
            <Typography variant="subtitle1" gutterBottom>
              <strong>User Stories:</strong>
            </Typography>
            {generatedStory.userStories?.map((x,y) => (
                <Paper
                  key={y}
                  sx={{
                    padding: 2,
                    backgroundColor: '#f9fbe7',
                    marginBottom: 2,
                    borderRadius: 1,
                  }}
                >
                  <Typography variant="subtitle2">
                    <strong>{x.key}'s Story:</strong>
                  </Typography>
                  <Typography>{x.value}</Typography>
                </Paper>
              )
            )}
          </Box>
        )}
      </Box>
    </Box>
  );
}

export default App;